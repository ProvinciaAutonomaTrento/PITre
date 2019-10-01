<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOutDocument.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.CheckOutDocument" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    var fsoApp;
    var finalfilePath = '';

    function confirmAction() {
        var retval = false;

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        try {
            disallowOp('Content1');

            if (CheckOutDocument('<%=DocumentId %>','<%=DocumentNumber %>',<%=CreateNewFile %>)) {
                retval = true;
            }
           
            reallowOp();
        }
        catch (ex) {
            alert(ex.toString());
            reallowOp();
            retval = false;
        }

        return retval;
    }


    function confirmActionSocket(callback) {
        var retval = false;
        try {
            disallowOp('Content1');


            (CheckOutDocumentSocket('<%=DocumentId %>','<%=DocumentNumber %>',<%=CreateNewFile %>, callback));
           

        }
        catch (ex) {
            alert(ex.toString());
            reallowOp();
        }
    }

    function SaveFileSystem() {
        var retval = false;
        var filePath = FisicalFilePath();
        
        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        if (filePath != null && filePath != '') {
            try {
                var encodedFilePath = EncodeHtml(filePath);
                var paramPost = "<<filePath:" + encodedFilePath + ">>";
                var urlPost = '<%=httpFullPath%>' + '/CheckInOutApplet/SaveFilePage.aspx';

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
                //ajaxDialogModal('File_DownloadError', 'error', '');
                alert('<%=this.GetMessage("DOWNLOAD_ERROR")%>:\n' + ex.toString());
                finalfilePath = '';
                retval = false;
            }
        }

        return retval;
    }

    function CheckOutDocument(idDocument, documentNumber, downloadFile) {
        var retValue = false;

        var filePath = FisicalFilePath();

        if (filePath != null && filePath != '') {
            try {
                // Validazione del formato file fornito in ingresso
                if (ValidateFileFormat) {
                    var encodedFilePath = EncodeHtml(filePath);

                    var status = 0;
                    var content = '';
                    $.ajax({
                        type: 'POST',
                        cache: false,
                        //dataType: "text",
                        processData: false,
                        url: "CheckOutPage.aspx?location=" + encodedFilePath + "&idDocument=" + idDocument + "&documentNumber=" + documentNumber + "&machineName=<%=MachineName%>&downloadFile=" + downloadFile,
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


                    var response = content; // http.responseText;

                    retValue = (response == "");

                    if (!retValue) {
                        alert(response);
                        retValue = false;
                    }

                    var downloaded = false;

                    if (retValue) {
                        // Download del file
                        downloaded = SaveFileSystem(); // DownloadCheckedOutDocument(filePath);

                        if (!downloaded)
                            throw new Error(0, "Impossibile scaricare il file '" + filePath + "'.\nIl file potrebbe non essere stato acquisito oppure potrebbe non essere presente alcun modello predefinito.");
                        else
                            finalfilePath = filePath;
                    }

                    // Validazione della dimensione del file fornito in ingresso

                    if (!retValue) {
                        alert(response);
                        retValue = false;
                    }
                    else {
                        retValue = true;
                    }
                }
            }
            catch (ex) {
                alert("Errore nel blocco del documento:\n" + ex.message.toString());

                // Nel caso l'operazione non è andata a buon fine, 
                // il blocco sul documento viene annullato (senza visualizzare messaggi)
                UndoCheckOutDocument(false, false, false);
                retValue = false;
            }
        }

        return retValue;
    }

    function SaveFileSystemSocket(callback) {
        var retval = false;
        FisicalFilePathSocket(function (filePath) {

            if (filePath != null && filePath != '') {
                try {
                    var encodedFilePath = EncodeHtml(filePath);


                    var paramPost = "<<filePath:" + encodedFilePath + ">>";
                    var urlPost = '../CheckInOutApplet/SaveFilePage.aspx?issocket=true';

                    $.ajax({
                        type: 'POST',
                        url: urlPost,
                        data: { "filePath": encodedFilePath },
                        success: function (content) {
                            saveFile(filePath, content, function (retVal, connection) {

                                if (retVal == "true") {
                                    openFile(filePath, function(ret, connection){
                                        callback(true);
                                        connection.close();
                                    });
                                        
                                }
                                else {
                                    alert('Applet error to get file.');
                                    callback(false);
                                }
                                connection.close();
                            });
                            

                        },
                        error: function () {
                            reallowOp();
                            callback(false);
                        },
                        async: true
                    });
                }
                catch (ex) {
                    //ajaxDialogModal('File_DownloadError', 'error', '');
                    alert('<%=this.GetMessage("DOWNLOAD_ERROR")%>:\n' + ex.toString());
                    retval = false;
                    reallowOp();
                    callback(retval);
                }
            }

        });

        

    }

    function CheckOutDocumentSocket(idDocument, documentNumber, downloadFile, callback) {
        var retValue = false;

        FisicalFilePathSocket(function(filePath){

        if (filePath != null && filePath != '') {
            try {
                // Validazione del formato file fornito in ingresso
                if (ValidateFileFormat) {
                    var encodedFilePath = EncodeHtml(filePath);

                    var status = 0;
                    var content = '';
                    $.ajax({
                        type: 'POST',
                        cache: false,
                        //dataType: "text",
                        processData: false,
                        url: "CheckOutPage.aspx?location=" + encodedFilePath + "&idDocument=" + idDocument + "&documentNumber=" + documentNumber + "&machineName=<%=MachineName%>&downloadFile=" + downloadFile,
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


                    var response = content; // http.responseText;

                    retValue = (response == "");

                    if (!retValue) {
                        alert(response);
                        retValue = false;
                    }

                    var downloaded = false;

                    if (retValue) {
                        // Download del file
                        SaveFileSystemSocket(function(downloaded){// DownloadCheckedOutDocument(filePath);

                                if (!downloaded){
                                    //throw new Error(0, "Impossibile scaricare il file '" + filePath + "'.\nIl file potrebbe non essere stato acquisito oppure potrebbe non essere presente alcun modello predefinito.");
                                    alert("Errore nel blocco del documento:\n Impossibile scaricare il file '" + filePath + "'.\nIl file potrebbe non essere stato acquisito oppure potrebbe non essere presente alcun modello predefinito.");
                                    UndoCheckOutDocumentSocket(false, false, false);
                                    callback(false);
                                }
                                else{
                                    finalfilePath = filePath;
                                    callback(true);
                                }
                            }); 
                    }else{
                        callback(false);
                    }

                }else{
                    callback(false);
                }

            }
            catch (ex) 
            {
                alert("Errore nel blocco del documento:\n" + ex.message.toString());

                // Nel caso l'operazione non è andata a buon fine, 
                // il blocco sul documento viene annullato (senza visualizzare messaggi)
                UndoCheckOutDocumentSocket(false, false, false);
                callback(false);
            }
        }else{
            callback(false);
        }
        });
    }


    function UndoCheckOutDocument() {
        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        try {
            var status = 0;
            var content = '';
            $.ajax({
                type: 'POST',
                cache: false,
                //dataType: "text",
                processData: false,
                url: "UndoCheckOutPage.aspx",
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


            var response = content;

            retValue = (response == "");

            if (!retValue) {
                // Visualizzazione messaggio di errore nell'undocheckout
                alert(response);
            }
            else {
                if (fsoApp.fileExists(finalfilePath)) {
                    try {
                        // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                        fsoApp.deleteFile(finalfilePath, true);
                    }
                    catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                    }
                }
            }
        }
        catch (ex) {
            alert("Errore nell'annullamento del blocco del documento:\n" + ex.message.toString());
        }
           
    }

function UndoCheckOutDocumentSocket() {
        
        try {
            var status = 0;
            var content = '';
            $.ajax({
                type: 'POST',
                cache: false,
                //dataType: "text",
                processData: false,
                url: "UndoCheckOutPage.aspx",
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


            var response = content;

            retValue = (response == "");

            if (!retValue) {
                // Visualizzazione messaggio di errore nell'undocheckout
                alert(response);
            }
            else {
                if(finalfilePath && finalfilePath !== ''){
                    fileExists(finalfilePath, function (ret, connection) {
                        try {
                            // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                            if(ret == 'true'){
                                deleteFile(finalfilePath, true);
                            }
                        }
                        catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                        }
                        connection.close();
                    });
                }
            }
        }
        catch (ex) {
            alert("Errore nell'annullamento del blocco del documento:\n" + ex.message.toString());
        }
           
    }

    // Validazione del formato file fornito in ingresso
    function ValidateFileFormat() {
        var formatValid = false;
        var formatValidForType = false;

        if ("<%=SupportedFileTypesEnabled%>" == "True") {
            var fileExtension = '<%=FileExtention %>';
            var tipoDocumento = "";
            var fileFormatsArray = "<%=SupportedFileFormats%>".split("|");

            // Se l'estensione non è valorizzata bisogna bypassare il controllo.
            // Serve per far funzionare i modelli M/Text
            if (fileExtension != null && fileExtension == '') {
                formatValid = true;
                formatValidForType = true;
            }

            if (fileFormatsArray != null && fileExtension != null && fileExtension != '') {
                for (var i = 0; i < fileFormatsArray.length; i++) {
                    var current = fileFormatsArray[i].split(':');

                    if (current[0].toUpperCase() == fileExtension.toUpperCase() && current[1] == "True") {
                        formatValid = true;

                        // Verifica se il formato file è valido per il tipo documento (grigio o protocollo)
                        tipoDocumento = "<%=TipoDocumento%>";
                        var fileValidFor = "<%=DocumentType%>".split("|")[i];

                        if ((fileValidFor == "Grigio" && tipoDocumento == "P") || (fileValidFor == "Protocollo" && tipoDocumento == "G"))
                            formatValidForType = false;
                        else
                            formatValidForType = true;
                        break;
                    }
                }
            }

            if (!formatValid)
                alert("Formato documento '" + fileExtension + "' non supportato.\nPer il supporto di questo formato rivolgersi all'amministratore di sistema.");
            else if (!formatValidForType) {
                var msg = "Formato '" + fileExtension + "' non valido per ";
                if (tipoDocumento == "P")
                    msg += "un documento protocollato";
                else
                    msg += "un documento grigio";
                alert(msg);
            }
        }
        else {
            formatValid = true;
            formatValidForType = true;
        }

        return (formatValid && formatValidForType);
    }
    
    function GetFileName() {
        var fileName = "";
        var ext = GetFileExtension();

        var txtName = document.getElementById("txtFileName").value;

        if (txtName != '') {
            if (ext.toUpperCase() == "P7M")
                fileName = txtName + "<%=this.GetP7mFileExtensions()%>";
            else
                fileName = txtName + "." + ext;
        }

        return fileName;
    }

    function FisicalFilePath() {
        var filePath = '';
        var folderPath = fixPath(document.getElementById("txtFolderPath").value);
        
        var tempFileName = GetFileName();
        if (tempFileName != '') {
            if (folderPath != null && folderPath != '') {
                if (fsoApp.folderExists(folderPath)) {
                    filePath = folderPath + tempFileName;
                }
                else {
                    if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                        if (fsoApp.createFolder(folderPath)) {
                            filePath = folderPath + tempFileName;
                        }
                        else {
                            ajaxDialogModal('Path_AccessDenied', 'error', '');
                            filePath = '';
                        }
                    }
                }
            }
            else {
                ajaxDialogModal('Path_Nonexistent', 'warning', '');
                filePath = '';
            }
        }
        else {
            ajaxDialogModal('File_InvalidName', 'warning', '');
            filePath = '';
        }

        return filePath;
    }

function FisicalFilePathSocket(callback) {
        var filePath = '';
        var folderPath = fixPath(document.getElementById("txtFolderPath").value);
        var tempFileName = GetFileName();
        if (tempFileName != '') {
            if (folderPath != null && folderPath != '') {
                folderExists(folderPath, function (retVal, connection) { 
                    if (retVal === "true") {
                        filePath = folderPath + tempFileName;
                        callback(filePath);
                    }
                    else {
                        if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                            createFolder(folderPath, function (retVal, connection) { 
                                if (retVal === "true") {
                                    filePath = folderPath + tempFileName;
                                    callback(filePath);
                                }
                                else {
                                    ajaxDialogModal('Path_AccessDenied', 'error', '');
                                    filePath = '';
                                    callback(filePath);
                                }
                                connection.close();
                            });
                        }
                    }
                    connection.close();
                });
            }
            else {
                ajaxDialogModal('Path_Nonexistent', 'warning', '');
                callback(filePath);
            }
        }
        else {
            ajaxDialogModal('File_InvalidName', 'warning', '');
            callback(filePath);
        }

    }

    function GetFileExtension() {
        var fileType = "";

        if (document.getElementById("cboFileTypes") != null)
            fileType = document.getElementById("cboFileTypes").value;
        else if (document.getElementById("lblFileType") != null)
            fileType = document.getElementById("lblFileType").innerHTML;

        return fileType;
    }

    function setTempFolder() {
        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        try {
            disallowOp('Content1');
            var folderPath = fsoApp.GetSpecialFolder();
            if (folderPath != null && folderPath != '') {
                document.getElementById("txtFolderPath").value = shortfolderPath(folderPath);
            }
            reallowOp();
        }
        catch (ex) {
            //alert(ex.toString());
            reallowOp();
        }
    }


    function setTempFolderSocket() {
		
		disallowOp('Content1');
        getSpecialFolder(function (folderPath, connection) {

            if (folderPath != null && folderPath != '') {
                document.getElementById("txtFolderPath").value = shortfolderPath(folderPath);
            }
            reallowOp();
            connection.close();
        });
    }
    function shortfolderPath(longpath) {
        var tempPath = "";
        var position = longpath.toUpperCase().indexOf("APPDATA");
        if (position > 0) {
            tempPath = longpath.substring(0, position) + "Documents";
        }
        else {
            tempPath = longpath;
        }

        return tempPath;
    }


    function SelectFolder() {

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        var actualFolder = document.getElementById("txtFolderPath").value;
        var folder = fsoApp.selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolder);

        if (folder != "") {
            if (fsoApp.folderExists(folder)) {
                document.getElementById("txtFolderPath").value = folder;
            }
            else {
                if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                    if (fsoApp.createFolder(folder)) {
                        document.getElementById("txtFolderPath").value = folder;
                    }
                    else {
                        ajaxDialogModal('Path_AccessDenied', 'error', '');
                        document.getElementById("txtFolderPath").value = '';
                    }
                }
            }
        }
    }

    function SelectFolderSocket() {
        var actualFolder = document.getElementById("txtFolderPath").value;
        selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolder,function(folder,connection){
            if (folder != "") {
                folderExists(folder, 
                    function(exist,connection){
                        if ("true" === exist) {
                            document.getElementById("txtFolderPath").value = folder;
                        }
                        else {
                            if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                                
                                createFolder(folder, function(create,connection){
                                    if ("true"===create) {
                                        document.getElementById("txtFolderPath").value = folder;
                                    }
                                    else {
                                        ajaxDialogModal('Path_AccessDenied', 'error', '');
                                        document.getElementById("txtFolderPath").value = '';
                                    }
                                    connection.close();
                                });
                            }
                        }
                        connection.close();
                    });
            }
            connection.close();
        });
    }

    function fixPath(tempPath) {
        strResult = '';
        totCh = tempPath.length;
        if (totCh > 0) {
            ch = tempPath.substring(totCh - 1, totCh);
            if (ch == '\\' || ch == '/') {
                strResult = tempPath;
            }
            else
            {
                if (navigator.platform.toUpperCase().indexOf('LINUX') !== -1 || navigator.platform.toUpperCase().indexOf('MAC') !== -1)
                    strResult = tempPath + '/';
                else
                    strResult = tempPath + '\\';
            }
        }

        return strResult;
    }

    function EncodeHtml(value) {
        value = escape(value);
        value = value.replace(/\//g, "%2F");
        value = value.replace(/\?/g, "%3F");
        value = value.replace(/=/g, "%3D");
        value = value.replace(/&/g, "%26");
        value = value.replace(/@/g, "%40");
        // Gabriele Melini 02-04-2014
        // bug lettere accentate
        value = value.replace(/%E0/g, "%C3%A0"); // à
        value = value.replace(/%E8/g, "%C3%A8"); // è
        value = value.replace(/%E9/g, "%C3%A9"); // é
        value = value.replace(/%EC/g, "%C3%AC"); // ì
        value = value.replace(/%F2/g, "%C3%B2"); // ò
        value = value.replace(/%F9/g, "%C3%B9"); // ù
        value = value.replace(/%A3/g, "%C2%A3"); // £
        value = value.replace(/%B0/g, "%C2%B0"); // °
        value = value.replace(/%A7/g, "%C2%A7"); // §
        return value;
    }

</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <asp:UpdatePanel ID="pnlAppletTag" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <applet id='fsoApplet' 
                code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
                codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                archive='FsoApplet.jar'
	        	width = '10'   height = '9'>
                <param name="java_arguments" value="-Xms128m" />
                <param name="java_arguments" value="-Xmx512m" />
            </applet>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
             <asp:UpdatePanel ID="udpFileSystem" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div><br />
                        <asp:Label ID="lblFolderPath" runat="server"></asp:Label><br />
                        <asp:TextBox ID="txtFolderPath" runat="server" Width="400px" ClientIDMode="Static"></asp:TextBox>
                        <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." ></asp:Button>
                    </div>
                    <br />
                    <div>
                        <asp:Label ID="lblFileName" runat="server"></asp:Label><br />
                        <asp:TextBox ID="txtFileName" runat="server" Width="210pt" ClientIDMode="Static"></asp:TextBox>
                        .<asp:Label ID="lblFileType" runat="server" ClientIDMode="Static"></asp:Label>
                        <asp:DropDownList ID="cboFileTypes" runat="server" Width="50pt" ClientIDMode="Static"></asp:DropDownList>
                    </div>
                    <asp:HiddenField ID="hdnOptSelected" runat="server" ClientIDMode="Static" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutConfirmButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CheckInOutConfirmButton_Click"/>
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="CheckInOutCloseButton_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>