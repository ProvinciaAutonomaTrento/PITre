<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOutModel.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.CheckOutModel" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    var fsoApp;
    var modelApp;
    var filePath;

    function confirmAction() {
        var retval = false;

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        filePath = FisicalFilePath();
        
        if (filePath != null && filePath != '') {
            try {
                idDocument = "<%=this.DocumentId%>";
                documentNumber = "<%=this.DocumentNumber%>";
                disallowOp('Content1');
                var modelProcessed = GetModelAndProcess(idDocument, "<%=this.ModelloDocumentoCorrente%>", filePath);
                if (modelProcessed != false) {
                    if (CheckOutDocument(idDocument, documentNumber, filePath)) {
                        if (fsoApp.openFile(filePath))
                            retval = true;
                    }
                    else
                        throw new Error("Impossibile bloccare il documento num. '" + documentNumber + "'.");
                }
                else {
                    throw new Error("Impossibile scaricare il file '" + filePath + "'.\nPotrebbe non essere presente alcun modello predefinito.");
                    retval = false;
                }
            }
            catch (ex) {
                alert(ex.toString());
                retval = false;
            }
        }
        else {
            alert('Selezionare path alido');
        }

        reallowOp();
        return retval;
    }

    function GetModelAndProcess(documentId, modelType, filePath) 
    {
        var retValue = false;
        if (modelApp == undefined) {
            modelApp = window.document.plugins[1];
        }
        if (modelApp == undefined) {
            modelApp = document.applets[1];
        }
        try {
                var status = 0;
                var content = '';
                $.ajax({
                    type: 'POST',
                    cache: false,
                    processData: false,
                    url: "GetXmlModelFromServer.aspx?documentId=" + documentId + "&modelType=" + modelType,
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

                if (content!=null && content!='') {
                    retValue = modelApp.processModel(documentId, modelType, content, filePath, false);
                }
                else {
                    alert("Impossibile contattare il ws...");
                }
        }
        catch (e) {
            alert(e.message.toString());
            retValue = false;
        }

        return retValue;
    }

    function CheckOutDocument(idDocument, documentNumber, filePath) {
        var retValue = false;

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        try {
            if (fsoApp.fileExists(filePath)) {
                var encodedFilePath = EncodeHtml(filePath);

                var status = 0;
                var content = '';
                $.ajax({
                    type: 'POST',
                    cache: false,
                    //dataType: "text",
                    processData: false,
                    url: "CheckOutPage.aspx?location=" + encodedFilePath + "&idDocument=" + idDocument + "&documentNumber=" + documentNumber + "&machineName=<%=MachineName%>&downloadFile=true",
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

                if (retValue) {
                    // Download del file
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

        return retValue;
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

    function fixPath(tempPath) {
        strResult = '';
        totCh = tempPath.length;
        if (totCh > 0) {
            ch = tempPath.substring(totCh - 1, totCh);
            if (ch == '\\' || ch == '/') {
                strResult = tempPath;
            }
            else {
                if (navigator.platform.toUpperCase().indexOf('LINUX') !== -1 || navigator.platform.toUpperCase().indexOf('MAC') !== -1)
                    strResult = tempPath + '/';
                else
                    strResult = tempPath + '\\';
            }
        }

        return strResult;
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


    function confirmActionSocket(callback) {
        var retval = false;
        disallowOp('Content1');
        FisicalFilePathSocket(function (filePath) {
            if (filePath != null && filePath != '') {
                try {
                    FileExists(function (existsFile) {
                        if (existsFile != true) {
                            idDocument = "<%=this.DocumentId%>";
                            documentNumber = "<%=this.DocumentNumber%>";
                            disallowOp('Content1');
                            GetModelAndProcessSocket(idDocument, "<%=this.ModelloDocumentoCorrente%>", filePath, function (modelProcessed) {
                                try {
                                    if (modelProcessed != false) {
                                        CheckOutDocumentSocket(idDocument, documentNumber, filePath, function (open) {
                                            if (open) {
                                                openFile(filePath,
                                                    function (msg, connection) {
                                                        connection.close();
                                                        reallowOp();
                                                        callback(true);
                                                    });
                                            }
                                            else
                                                throw new Error("Impossibile bloccare il documento num. '" + documentNumber + "'.");
                                        });
                                    }
                                    else {
                                        throw new Error("Impossibile scaricare il file '" + filePath + "'.\nPotrebbe non essere presente alcun modello predefinito.");
                                    }
                                }
                                catch (ex) {
                                    alert(ex.toString());
                                    reallowOp();
                                    callback(false);
                                }
                            });
                        }
                        else
                        {
                            reallowOp();
                        }
                    });
                }
                catch (ex) {
                    alert(ex.toString());
                    retval = false;
                    reallowOp();
                }
            }
            else {
                alert('Selezionare path valido');
            }
        });
    }

    function GetModelAndProcessSocket(documentId, modelType, filePath, callback) {
        var retValue = false;
        try {
            var status = 0;
            var content = '';
            $.ajax({
                type: 'POST',
                cache: false,
                processData: false,
                url: "GetXmlModelFromServer.aspx?documentId=" + documentId + "&modelType=" + modelType,
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

            if (content != null && content != '') {
                processModel(documentId, modelType, content, filePath, false, function (ret, connection) {
                    if (ret === 'true')
                        callback(true);
                    else
                        callback(false);

                    connection.close();
                });
            }
            else {
                alert("Impossibile contattare il ws...");
                callback(false);
            }
        }
        catch (e) {
            alert(e.message.toString());
            callback(false);
        }
    }

    function CheckOutDocumentSocket(idDocument, documentNumber, filePath, callback) {
        var retValue = false;
        try {
            fileExists(filePath, function (retVal, connection) {
                if (retVal === 'true') {

                    var encodedFilePath = EncodeHtml(filePath);

                    var status = 0;
                    var content = '';
                    $.ajax({
                        type: 'POST',
                        cache: false,
                        //dataType: "text",
                        processData: false,
                        url: "CheckOutPage.aspx?location=" + encodedFilePath + "&idDocument=" + idDocument + "&documentNumber=" + documentNumber + "&machineName=<%=MachineName%>&downloadFile=true",
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

                    if (retValue) {
                        // Download del file
                    }

                    // Validazione della dimensione del file fornito in ingresso

                    if (!retValue) {
                        alert(response);
                        retValue = false;
                    }
                    else {
                        retValue = true;
                    }
                    callback(retValue);
                }
                connection.close();
            });
        }
        catch (ex) {
            alert("Errore nel blocco del documento:\n" + ex.message.toString());

            // Nel caso l'operazione non è andata a buon fine, 
            // il blocco sul documento viene annullato (senza visualizzare messaggi)
            UndoCheckOutDocumentSocket();
            retValue = false;
            callback(retValue);
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
                fileExists(finalfilePath, function (retVal, connection) {
                    if (retVal === 'true') {
                        try {
                            // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                            deleteFile(finalfilePath, true);
                        }
                        catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                        }
                    }
                    connection.close();
                });

            }
        }
        catch (ex) {
            alert("Errore nell'annullamento del blocco del documento:\n" + ex.message.toString());
        }

    }

    function FileExists(callback) {
        var filePath = '';
        var folderPath = fixPath(document.getElementById("txtFolderPath").value);
        var tempFileName = GetFileName();
        if (tempFileName != '') {
            if (folderPath != null && folderPath != '') {
                fileExists(folderPath + tempFileName, function (retVal, connection) {
                    if (retVal === "true") {
                        ajaxDialogModal('File_Exists', 'error', '');
                        filePath = '';
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }
                    connection.close();
                });
            }
            else {
                ajaxDialogModal('Path_Nonexistent', 'warning', '');
                callback(false);
            }
        }
        else {
            ajaxDialogModal('File_InvalidName', 'warning', '');
            callback(false);
        }
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

    function GetFileExtensionSocket() {
        var fileType = "";

        if (document.getElementById("cboFileTypes") != null)
            fileType = document.getElementById("cboFileTypes").value;
        else if (document.getElementById("lblFileType") != null)
            fileType = document.getElementById("lblFileType").innerHTML;

        return fileType;
    }

    function setTempFolderSocket() {
        try {
            disallowOp('Content1');


            getSpecialFolder(function (folderPath, connection) {

                if (folderPath != null && folderPath != '') {
                    document.getElementById("txtFolderPath").value = shortfolderPath(folderPath);
                }
                reallowOp();
                connection.close();
            });

        }
        catch (ex) {
            //alert(ex.toString());
            reallowOp();
        }
    }


    function SelectFolderSocket() {
        var actualFolder = document.getElementById("txtFolderPath").value;
        selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolder, function (folder, connection) {

            if (folder != "") {


                folderExists(folder, function (retVal, connection) {

                    if (retVal === 'true') {
                        document.getElementById("txtFolderPath").value = folder;
                    }
                    else {
                        if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {

                            createFolder(folder, function (retVal, connection) {

                                if (retVal == 'true') {
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

    function EncodeHtml(value) {
        value = escape(value);
        value = value.replace(/\//g, "%2F");
        value = value.replace(/\?/g, "%3F");
        value = value.replace(/=/g, "%3D");
        value = value.replace(/&/g, "%26");
        value = value.replace(/@/g, "%40");
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
            <applet id='modelApp' 
                code = 'Models.ModelProcessor' 
                codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                archive='ModelApplet.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>Aspose.Words.jdk15.jar'
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
                        <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." OnClientClick="SelectFolder();"></asp:Button>
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
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CheckInOutConfirmButton_Click" />
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="CheckInOutCloseButton_Click" OnClientClick="parent.closeAjaxModal('CheckOutModelApplet','');"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>