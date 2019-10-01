<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadFile.aspx.cs" Inherits="NttDataWA.Popup.UploadFile"
    MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Css/FSOFileUploader.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        var applet = undefined;
        var applet2 = undefined;

        function openSocket() {

            var fileType = "0";
            var applyImageCompression = "true";
            var pdfCompressionLevel = "1.07f";

            <%if (PdfConversionSynchronousLC)
        {%>
            fileType = "1";
            <%}
        else
        {%>
            if (document.getElementById("ContentPlaceHolderContent_fileOptions_0").checked)
                fileType = "1";
            <%} %>

            disallowOp('Content2');

            try {

                initScanningDialog(applyImageCompression, pdfCompressionLevel, fileType, function (retPath, connection) {

                    if (retPath == "0") {
                        reallowOp();
                        //alert('<asp:Literal id="litCancelNA" runat="server" />');
                        connection.close();
                    }
                    else if (retPath == "-1") {
                        reallowOp();
                        alert('<asp:Literal id="litNoAppletError" runat="server" />');
                        connection.close();
                    }
                    else {
                        var cartaceo = true;
                        var url = '../SmartClient/UploadPageHandler.aspx?cartaceo=' + cartaceo;
                        var sessId = '<%=System.Web.HttpContext.Current.Session.SessionID%>';

                        getFileFromPath(retPath, url, function (getFile, connection) {
                            //alert(getFile);

                            function sendError() {
                                alert('<asp:Literal id="litSendErrorNA" runat="server" />');
                            }

                            $.ajax({
                                type: 'POST',
                                url: url,
                                data: 'contentFile=' + getFile,
                                success: function () {
                                    reallowOp();
                                    parent.closeAjaxModal('UplodadFile', 'up');
                                },
                                error: function () {
                                    reallowOp();
                                    sendError();
                                },
                                async: true
                            });

                            connection.close();
                        });
                    }
                    connection.close();
                });
            }
            catch (err) {
                reallowOp();
                alert("Error:" + err.description);
            }
        }


        function openApplet() {
            var fileType = "0";

    <%if (PdfConversionSynchronousLC)
        {%>
            fileType = "1";
    <%}
        else
        {%>
            if (document.getElementById("ContentPlaceHolderContent_fileOptions_0").checked)
                fileType = "1";
    <%} %>

            disallowOp('Content2');

            try {

                if (applet == undefined) {
                    applet = window.document.plugins[0];
                }

                if (applet == undefined) {
                    applet = document.applets[0];
                }

                if (applet2 == undefined) {
                    applet2 = window.document.plugins[1];
                }

                if (applet2 == undefined) {
                    applet2 = document.applets[1];
                }

                applet.setFileType(fileType);
                var retPath = applet.initScanningDialog();

                if (retPath == "0") {
                    reallowOp();
                    //alert('<asp:Literal id="litCancel" runat="server" />');
                }
                else if (retPath == "-1") {
                    reallowOp();
                    alert('<asp:Literal id="litAppletError" runat="server" />');
                }
                else {
                    var cartaceo = true;
                    var url = '<%=fullpath%>/SmartClient/UploadPageHandler.aspx?cartaceo=' + cartaceo;
                    var sessId = '<%=System.Web.HttpContext.Current.Session.SessionID%>';
                    var retVal = applet2.sendFile(retPath, url, sessId);

                    if (retVal) {
                        reallowOp();
                        //alert('<asp:Literal id="litSendOk" runat="server" />');
                        applet.closeApplet();
                        applet2.closeApplet();
                        try {
                            //applet.killApplet();
                        }
                        catch (err) {
                        }

                        parent.closeAjaxModal('UplodadFile', 'up');
                    }
                    else {
                        reallowOp();
                        alert('<asp:Literal id="litSendError" runat="server" />');
                    }
                }
            }
            catch (err) {
                reallowOp();
                alert("Error:" + err.description);
            }

            return false;
        }

        function openSmartClient() {
            try {
                disallowOp('Content2');

                var acquisizione = document.getElementById("myControl");
                acquisizione.ApplyImageCompression = true;

        <%if (PdfConversionSynchronousLC)
        {%>
                acquisizione.ApplyPdfConvertion = true;
        <%}
        else
        {%>
                acquisizione.ApplyPdfConvertion = document.getElementById("ContentPlaceHolderContent_fileOptions_0").checked;
        <%} %>

                acquisizione.AllowScan = true;
                acquisizione.AllowSave = true;
                acquisizione.ShowRemovePage = true;
                acquisizione.ShowPageNavigations = true;
                acquisizione.ShowZoomCapabilities = true;
                acquisizione.Title = '<asp:Literal id="litTitle" runat="server" />';
                acquisizione.SetSizeScreenPercentage(100, 100);
                acquisizione.Signature = "";
                var ret = acquisizione.ShowImageViewer();

                var imagePath = "";

                if (ret) {
                    imagePath = acquisizione.ImagePath;
                    //self.returnValue = imagePath;
                    //self.close();
                    if (imagePath != null && imagePath != '') {
                        var pdfAcrobat = (IsConvertPdfWithAcrobat());
                        if (pdfAcrobat) {
                            // Se conversione in pdf con sdk di adobe acrobat attiva,
                            // viene fatta la conversione in pdf del file tiff
                            // e inviato il file a docspa
                            ConvertPdfWithAcrobat(imagePath, false);
                        }
                        else {
                            // Upload del file
                            var risultato = (InviaFileXmlUpload(imagePath, document.getElementById("ContentPlaceHolderContent_fileOptions_0").checked, true, true, false, false));
                            if (risultato) {
                                try {
                                    var fso = FsoWrapper_CreateFsoObject();
                                    if (fso.FileExists(imagePath))
                                        fso.DeleteFile(imagePath);
                                }
                                catch (ex) {

                                }
                                reallowOp();
                                parent.closeAjaxModal('UplodadFile', 'up');
                            }
                            else {
                                reallowOp();
                                alert('<asp:Literal id="litUploadError" runat="server" />\n' + imagePath);
                            }
                        }
                    }
                }
                else {
                    reallowOp();
                    alert('<asp:Literal id="litCancel2" runat="server" />');
                }

            }
            catch (ex) {
                alert('<asp:Literal id="litActiveXError" runat="server" />:\n' + ex.message.toString());

                //self.returnValue = "";
                //self.close();
                reallowOp();
            }

            return false;
        }

        function InviaFileXmlUpload(fileName, convertiPDF, keepOriginal, removeLocalFile, convertiPDFServer, convertPdfSync) {
            var uploader = null;
            var xml_dom = null;
            var cartaceo = true;
            var retval = false;

            try {
                uploader = new ActiveXObject("DocsPa_AcquisisciDoc.ctrlUploader");
                xml_dom = uploader.GetXMLRequest(fileName, convertiPDF && !convertiPDFServer, false);
            } catch (ex) {
                alert('<asp:Literal id="litUploadError2" runat="server" />:\n' + ex.Description);
                retval = false;
                return retval;
            }

            if (uploader.ErrNumber != 0) {
                alert(uploader.ErrDescription);
                retval = false;
            }
            else {
                var http = null;
                try {
                    http = new ActiveXObject("MSXML2.XMLHTTP");
                    http.Open("POST", "<%= fullpath%>/Upload.aspx?cartaceo=" + cartaceo + "&convertiPDF=" + convertiPDF + "&convertiPDFServer=" + convertiPDFServer + "&convertiPDFServerSincrono=" + convertPdfSync, false);
                    http.send(xml_dom);
                    retval = true;
                } catch (ex) {
                    alert('<asp:Literal id="litUploadError3" runat="server" />:\n' + ex.Message);
                    retval = false;
                    return retval;
                }
            }

            return retval;
        }

        function ConvertPdfWithAcrobat(originalFilePath, recognizeText) {
            var outputPDFFilePath = GetTemporaryPDFFilePath();

            // Viene avviato il processo si conversione con l'integrazione adobe				
            if (ConvertPdfFile(originalFilePath, outputPDFFilePath, recognizeText))
                InviaFileXmlUpload(outputPDFFilePath, false, false, false, false, false);

        }

        function ConvertPdfFile(inputFile, outputFile, recognizeText) {
            try {
                var retValue = false;

                if (IsIntegrationActiveAndInstalled()) {
                    var acrobatInterop = CreateObject("<%=AcrobatIntegrationClassId%>");
                    retValue = acrobatInterop.ConvertFileToPDF(inputFile,
                        outputFile,
                        recognizeText);
                }

                return retValue;
            }
            catch (ex) {
                throw ('<asp:Literal id="litPDFError" runat="server" />:\n' + ex.message.toString());
            }
        }

        function IsConvertPdfWithAcrobat() {
            var acrobatEnabled = ("<%=IsAdobeIntegrationActive%>" == "True");
            var pdfchecked = document.getElementById("ContentPlaceHolderContent_fileOptions_0").checked;
            var installedIntegration = IsInstalledAdobeAcrobatIntegration();

            return (acrobatEnabled && pdfchecked && installedIntegration);
        }

        function IsInstalledAdobeAcrobatIntegration() {
            var retValue = false;

            try {
                var acrobatInterop = new ActiveXObject("<%=AcrobatIntegrationClassId%>");
                retValue = true;
            }
            catch (e) {
                retValue = false;
            }

            return retValue;
        }

        function ScanWithAcrobatIntegration() {
            return ("<%=ScanWithAcrobatIntegration%>" == "True");
        }

        function GetTemporaryPDFFilePath() {
            var fso = FsoWrapper_CreateFsoObject();
            // OLD:  var pdfFolder=fso.GetSpecialFolderPath(2) + "\\DPAImage\\";
            var specialFolder = "";
            try {

                specialFolder = fso.GetSpecialFolderPath(2);
            }
            catch (e) {

                specialFolder = fso.GetSpecialFolder(2);
            }

            var pdfFolder = specialFolder + "\\DPAImage\\";

            if (!fso.FolderExists(pdfFolder))
                fso.CreateFolder(pdfFolder);

            return pdfFolder + fso.GetTempName() + ".pdf";
        }

        function ScanPdfFile(outputFile, recognizeText) {
            try {
                var retValue = false;

                if (IsIntegrationActiveAndInstalled()) {
                    var acrobatInterop = CreateObject("<%=AcrobatIntegrationClassId%>");

                    if ("<%=IsAcrobat6Interop%>" == "True")
                        retValue = acrobatInterop.AcquireFromScanner(outputFile, recognizeText);
                    else
                        retValue = acrobatInterop.AcquireFromScanner(outputFile);
                }

                return retValue;
            }
            catch (e) {
                throw ('<asp:Literal id="litScanError" runat="server" />:\n' + e.message.toString());
            }

            return retValue;
        }

        function openActiveX() {
            parent.ajaxModalPopupActiveXScann();
            parent.closeAjaxModal('UplodadFile', '');

            return false;
        }

        function ShowImageViewer() {
            //myControl = document.getElementById("myControl");

            document.myControl.ImageViewer.AllowScan = true;
            document.myControl.ImageViewer.ShowOpenFile = false;
            document.myControl.ImageViewer.AllowSave = true;
            document.myControl.TitleSeparateWindow = '<asp:Literal id="litTitle2" runat="server" />';
            document.myControl.ShowOnSeparateWindow = true;

            document.myControl.SetSizeScreenPercentage(100, 100);

            var ret = document.myControl.ShowImageViewer();

            // Restituzione del percorso del file acquisito
            var imagePath = document.myControl.ImageViewer.ImagePath;

            // Chiusura immagine correntemente visualizzata
            myControl.ImageViewer.CloseImage();

            if (!ret && imagePath != "") {
                var fso = FsoWrapper_CreateFsoObject();
                if (fso.FileExists(imagePath))
                    fso.DeleteFile(imagePath);

                imagePath = "";
            }

            return imagePath;
        }

        /* FSO File uploader region */
        MessageStatus = {
            Success: 1,
            Information: 2,
            Warning: 3,
            Error: 4
        }

        //Enumeration for messages status class
        MessageCSS = {
            Success: "Success",
            Information: "Information",
            Warning: "Warning",
            Error: "Error"
        }

        //Global variables
        var intervalID = 0;
        var subintervalID = 0;
        var fileUpload;
        var form;
        var previousClass = '';

        //Attach to the upload click event and grab a reference to the progress bar
        function pageLoad() {
            //$addHandler($get('upload'), 'click', onUploadClick);
        }

        //Register the form
        function register(form, fileUpload) {
            this.form = form;
            this.fileUpload = fileUpload;
        }

        //Start upload process
        function onUploadClick() {
            ////console.log('Invocato onUploadClick');
            if (fileUpload.value.length > 0) {
                var filename = '';
                if (filename == '') {
                    //Update the message
                    updateMessage(MessageStatus.Information, '<asp:Literal id="litSending" runat="server" />', '', '<asp:Literal id="litStartDownloadBytes" runat="server" />');
                    //Submit the form containing the fileupload control
                    form.submit();
                    disallowOp('Content2');
                    //Initialize progressbar
                    setProgress(0);
                    //Start polling to check on the progress ...
                    //startProgress();

                    PageMethods.InitUpload();

                    intervalID = window.setInterval(function () {
                        PageMethods.GetUploadStatus(function (result) {
                            if (result) {
                                setProgress(result.percentComplete);
                                //Upadte the message every 500 milisecond
                                updateMessage(MessageStatus.Information, result.message, result.fileName, result.downloadBytes);
                                if (result == 100 || result.percentComplete == 100) {
                                    endProgress(result.fileName, result.downloadBytes);
                                }
                            }
                        }, function (err) {
                            alert(err.get_message());
                        });
                    }, 500);
                }
                else {
                    onComplete(MessageStatus.Error, '<strong>' + filename + '</strong> <asp:Literal id="litExists" runat="server" />', '', '<asp:Literal id="litStartDownloadBytes2" runat="server" />');
                }
            }
            else {
                updateMessage(MessageStatus.Warning, '<asp:Literal id="litNone" runat="server" />', '', '<asp:Literal id="litStartDownloadBytes3" runat="server" />')
            }

            return false;
        }

        //Stop progrss when file was successfully uploaded
        function onComplete(type, msg, filename, downloadBytes) {
            //alert('Chiamata onComplete');
            window.clearInterval(intervalID);
            clearTimeout(subintervalID);
            updateMessage(type, msg, filename, downloadBytes);
            if (type == MessageStatus.Success) {
                setProgress(100);
                //enableSend();
            }
            reallowOp();
        }

        //Update message based on status
        function updateMessage(type, message, filename, downloadBytes) {
            $get('dvDownload').innerHTML = downloadBytes;
            $get('dvFileName').innerHTML = filename;
        }

        //Set progressbar based on completion value
        function setProgress(completed) {
            $get('dvProgressPrcent').innerHTML = completed + '%';
            $get('dvProgress').style.width = completed + '%';
        }

        //This will call every 200 milisecnd and update the progress based on value
        function startProgress() {
            var increase = $get('dvProgressPrcent').innerHTML.replace('%', '');
            increase = Number(increase) + 1;
            if (increase <= 100) {
                setProgress(increase);
                subintervalID = setTimeout("startProgress()", 200);
            }
            else {
                window.clearInterval(subintervalID);
                clearTimeout(subintervalID);
            }
        }

        function endProgress() {
            var filename = '';
            var downloadBytes = '';

            if (arguments.length > 0 && arguments[0] != null)
                filename = arguments[0];
            else
                filename = result.fileName;

            if (arguments.length > 1 && arguments[1] != null)
                downloadBytes = Math.round(parseInt(arguments[1]) / 1024) + ' KB';
            else
                downloadBytes = Math.round(parseInt(result.downloadBytes) / 1024) + ' KB';

            window.clearInterval(intervalID);
            clearTimeout(subintervalID);
            updateMessage(MessageStatus.Information, '<asp:Literal id="litCompleted" runat="server" />', filename, downloadBytes);
            setProgress(100);
            $get('<%=UploadBtnUploadFile.ClientID %>').className = 'btnEnable';
            $get('<%=UploadBtnUploadFile.ClientID %>').disabled = false;
            reallowOp();
        }
        /* End region */


        $(document).ready(function () {

            

        });


    </script>
    <style type="text/css">
        fieldset.uploadFile, fieldset.uploadFile2 {
            border: 0;
            padding: 10px;
            margin: 10px auto;
            display: block;
            white-space: nowrap;
            font-size: 1.4em;
            width: 250px;
        }

        div.uploadFile {
            margin: 10px 0 10px 0;
            height: 70px;
        }

            div.uploadFile p {
                text-align: center;
                margin: 10px auto 0 auto;
            }

            div.uploadFile p {
                color: #999;
                font-size: 0.6em;
            }

        p.otherwise {
            text-align: center;
            color: #999;
            font-size: 0.8em;
            height: auto;
        }

        fieldset.uploadFile label {
            display: inline;
            margin: 0 0 0 20px;
        }

        fieldset.uploadFile2 {
            font-size: 0.9em;
            margin: 0 auto;
            height: 20px;
        }

        fieldset.uploadFile3 {
            width: 400px;
            margin: 0 auto;
            border: 0;
        }

            fieldset.uploadFile3 p {
                text-align: center;
                margin: 0 auto 5px auto;
                color: #999;
                font-size: 1.2em;
            }

                fieldset.uploadFile3 p span {
                    font-size: 0.8em;
                }

        #dvFileName, #dvDownload, #dvProgressPrcent {
            color: #999;
            font-size: 0.8em;
            border: 0;
        }

        #dvProgressContainer {
            width: 100%;
            background: url(../Images/Uploader/progress-deactive.png) repeat-x top left;
            height: 34px;
        }

        #dvProgress {
            width: 0;
            background: url(../Images/Uploader/progress-active.png) repeat-x top left;
            height: 34px;
        }
    </style>

    <style type="text/css">
        table#uploadBox {
            width: 90%;
            margin: 0 auto;
            font-size: 1.2em;
        }

            table#uploadBox td {
                vertical-align: middle;
            }

                table#uploadBox td p {
                    text-align: center;
                }

                table#uploadBox td label {
                    margin-left: 15px;
                }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <%if (componentType == NttDataWA.Utils.Constans.TYPE_ACTIVEX)
        { %>
    <!--ACTIVEX-->
    <%}
        else if (componentType == NttDataWA.Utils.Constans.TYPE_SMARTCLIENT)
        {%>
    <uc1:FsoWrapper ID="fsoWrapper" runat="server" />
    <object id="myControl" height="0" width="0"
        data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
        classid="../SmartClient/Librerie/DPA.Web.dll#DPA.Web.Imaging.ImageViewerContainerControl"
        viewastext>
    </object>
    <object id="ctrlUploader" codebase="../activex/DocsPa_AcquisisciDoc.CAB#version=1,0,0,0"
        classid="CLSID:27AEF6CF-0C73-4772-B6CD-F904A469184D" viewastext>
        <param name="_ExtentX" value="0">
        <param name="_ExtentY" value="0">
    </object>
    <%}
        else if (componentType == NttDataWA.Utils.Constans.TYPE_APPLET)
        {%>
    <asp:PlaceHolder ID="plcApplet" runat="server">
        <applet id='scanApplet'
            code='com.nttdata.scannerApplet.gui.ScanApplet'
            codebase='<%=Page.ResolveClientUrl("~/Libraries/")%>'
            archive='ScanningApplet.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>morena.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>morena_windows.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>morena_osx.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>morena_license.jar, <%=Page.ResolveClientUrl("~/Libraries/Libs/")%>itext-5.0.6.jar, <%=Page.ResolveClientUrl("~/Libraries/Libs/")%>jai_codec.jar, <%=Page.ResolveClientUrl("~/Libraries/Libs/")%>jai_core.jar'
            width='10' height='9'>
            <param name="ApplyImageCompression" value="true" />
            <param name="PdfCompressionLevel" value="0.97f" />
            <param name="java_arguments" value="-Xms128m" />
            <param name="java_arguments" value="-Xmx1024m" />
        </applet>
        <applet id='fsoApplet'
            code='com.nttdata.fsoApplet.gui.UploaderManager'
            codebase='<%=Page.ResolveClientUrl("~/Libraries/")%>'
            archive='FsoApplet.jar'
            width='10' height='9'>
            <param name="java_arguments" value="-Xms128m" />
            <param name="java_arguments" value="-Xmx1024m" />
        </applet>
    </asp:PlaceHolder>
    <%} %>




    <asp:UpdatePanel ID="upPnlGeneral" runat="server">
        <ContentTemplate>
            <style>

                .customEnableButton {
                    padding: 8px 5px;
                    background-size: cover;
                    background-position-y: center;
                    border-radius: 5px;
                    font-weight: bold;
                    border: 1px solid #3377ff;
                    margin: 0 !important;
                }


                div#accordion {
                    margin: 5px auto !important;
                    width: 97% !important;
                }

                    div#accordion > h3 {
                        padding: 10px 0 10px 25px !important;
                    }

                .accordionContent {
                    background-color: white;
                    margin: 0;
                    padding: 10px;
                    top: 0;
                }

                    .accordionContent > div {
                        text-align: center;
                        width: 100%;
                        height: 100%;
                        padding: 0;
                        margin: 0;
                    }


                        .accordionContent > div > div {
                            display: table-cell;
                            vertical-align: middle;
                            text-align: center;
                        }

                            .accordionContent  input {
                                margin: 0 auto !important;
                            }


            </style>

            <div id="accordion">

                <!-- SECTION 1 -->
                <!-- Upload Massivo  -->
                <h3 class="ie9HiddenField"><asp:Literal ID="labelCaptionUploadMassivo" runat="server" /></h3>
                <div class="accordionContent ie9HiddenField">
                     <table style="width:100%; text-align: center;">
                         <tr>
                            <th style="width: 50%;">
                                <asp:Literal ID="labelCellHeaderUploadDocument" runat="server"></asp:Literal></th>
                            <th id="cellLabelAttachment" runat="server" clientidmode="Static" style="width: 50%;">Allegati</th> 
                          </tr>
                         <tr>
                             <!-- Documento Principale -->
                             <td>
                                 <div id="uploadFileDocumentMassivo_box" style="margin: 5px;">
                                    <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important;">
                                        <label id="btnUploadFileDocumentMassivo" class="fileinput-button">
                                            <span><asp:Literal runat="server" ID="labelButtonUploadDocument"></asp:Literal></span>
                                        </label>
                                        <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                    </div>
                                    <div class="fileupload-content" style="margin: 0 7px 0 5px;">
                                        <table class="files"></table>
                                    </div>
                                </div>
                             </td>
                             <!-- Allegati-->
                             <td id="cellUploadAttachment" runat="server" clientidmode="Static">
                                 <div id="uploadAttachmentsMassivo_box" >
                                     <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important;">
                                        <label id="btnAttachmentstMassivo" class="fileinput-button">
                                            <span>Acquisisci Allegati</span>
                                        </label>
                                        <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                    </div>
                                    <div class="fileupload-content" style="margin: 0 7px 0 5px;">
                                        <table class="files"></table>
                                    </div>
                                 </div>
                             </td>
                         </tr>
                         
                         <tr>
                             <td style="width:50%;">
                                 <p id="_txtFileDocumentSelect" style="word-break: break-all;"></p>
                             </td>
                             <td>
                                 <p id="_txtFileAttachmentsSelect" style="word-break: break-all;"></p>
                             </td>
                         </tr>
                         <tr>
                             <td colspan="2">
                                 <div id="boxUploadErrorLog" style="max-height: 50px; overflow-y:auto;"></div>
                             </td>
                         </tr>
                         <tr>
                             <td colspan="2">
                                 <!-- Conversione PDF -->
                                 <input id="cbxUploadConvertiPdf" type="checkbox" value="pdf" name="ConversionePdf" /> Converti in PDF (sincrono)
                                 <input id="cbxUploadCartaceo" type="checkbox" value="cartaceo" name="ConversionePdf" /> Cartaceo
                             </td>
                         </tr>
                        <tr>
                            <td colspan="2">
                                <label id="btnStartUploadMassivo" style="display: none;" class="btnEnable customEnableButton">
                                    <span class="ui-button-text">Acquisisci</span>
                                </label>
                                <div id="btnStartUploadAttachmentMassivo" style="height: 1px; width: 1px; background-color:transparent;">
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                

                <!-- SECTION 2 -->
                <!-- Acquisizione Documenti Vecchia -->
                <h3 class="ie9ShowedField">Acquisisci Documento</h3>
                <div class="accordionContent ie9ShowedField" style="min-height: 300px;">
                    <div class="uploadFile">
                        <asp:UpdatePanel ID="UpdatePanelFsoFile" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <p>
                                    <iframe id="uploadFrame" frameborder="0" height="30" width="100%" scrolling="no" src="../SmartClient/UploadEngine.aspx"></iframe>
                                    <br />
                                    <span>
                                        <asp:Literal ID="litUploadMax" runat="server" /></span>
                                </p>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <fieldset class="uploadFile3">
                            <p>
                                <asp:Literal ID="litStatus" runat="server" /><br />
                                <span>
                                    <asp:Literal ID="litDownloadBytes" runat="server" /></span>
                            </p>
                            <div>
                                <table cellpadding="0" cellspacing="2" width="100%" border="0">
                                    <tr>
                                        <td style="width: 100%;">
                                            <table cellpadding="0" cellspacing="0" width="100%" border="0">
                                                <tr>
                                                    <td align="left" style="width: 90%;">
                                                        <div id="dvProgressContainer">
                                                            <div id="dvProgress"></div>
                                                        </div>
                                                    </td>
                                                    <td align="right">
                                                        <span id="dvProgressPrcent">0%</span>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <span id="dvFileName"></span><span id="dvDownload">
                                                <asp:Literal ID="litStartDownloadBytes4" runat="server" /></span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </fieldset>
                        <fieldset class="uploadFile2">
                            <asp:CheckBoxList id="fileOptions" runat="server" RepeatDirection="Horizontal" CssClass="checklist">
                                <asp:ListItem id="optPDF"></asp:ListItem>
                                <%--<asp:ListItem id="optPapery"></asp:ListItem>--%>
                            </asp:CheckBoxList>
                        </fieldset>
                        <cc1:CustomButton ID="UploadBtnUploadFile" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                            OnMouseOver="btnHover" ClientIDMode="Static" OnClick="UploadBtnUploadFile_Click" Enabled="false" OnClientClick="disallowOp('Content2');" />
                    </div>
                </div>
                

                <!-- SECTION 3 -->
                <!-- Acquisizione da SCANNER -->
                <h3><asp:Literal ID="labelCaptionScanner" runat="server" /></h3>
                <div class="accordionContent">
                    <div>
                            <cc1:CustomButton ID="scanAcquire" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
                        <div style="padding: 10px;">
                            <input id="cbxScannerConvertiPdf" type="checkbox" value="pdf" name="ConversionePdf" /> Converti in PDF (sincrono)
                        </div>
                    </div>
                </div>

                
                <!-- SECTION 4 -->
                <!-- Upload Big File -->
                <h3 id="headerAccordionBigFile" runat="server"><asp:Literal ID="labelCaptionBigFile" runat="server" /></h3>
                <div id="contentAccordionBigFile" runat="server" class="accordionContent">
                    <div>
                        <cc1:CustomButton ID="repositoryOpen" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="parent.closeAjaxModal('UplodadFile', 'repository');" />
                    </div>
                    
                </div>
             

            </div>

            <table style="width:100%; text-align: center;">
                <tr>
                    <td colspan="2">
                        <label id="btnClosePanel" style="display: none;" class="btnEnable customEnableButton">
                            <span class="ui-button-text">Chiudi</span>
                        </label>
                    </td>
                </tr>
            </table>


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

            <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.tmpl.js?v=3") %>" type="text/javascript"></script>
            <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.tmplPlus.js") %>" type="text/javascript"></script>
            <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.iframe-transport.js") %>" type="text/javascript"></script>
            <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.fileupload.js") %>" type="text/javascript"></script>
            <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.fileupload-ui.js") %>" type="text/javascript"></script>
            <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/UploadFileToDocument.js?v=56") %>" type="text/javascript"></script>

            <script type="text/javascript">
                $(document).ready(function () {
                    try {
                        $("#footer").hide();
                        $("#container").css('bottom', '0px');
                    } catch (error) {
                       //console.log(error);
                    }

                });
            </script>


         
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" Visible="true" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons" UpdateMode="Conditional" Visible="true">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



<%--<asp:content id="ContentCustomScript" runat="server" >
</asp:content>--%>