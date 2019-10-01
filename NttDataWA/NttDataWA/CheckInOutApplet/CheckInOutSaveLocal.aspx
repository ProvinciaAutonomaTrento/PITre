<%@ Page Language="C#" CodeBehind="CheckInOutSaveLocal.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.CheckInOutSaveLocal" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var fsoApp;
        var specialExtension = '';

        function confirmAction() {
            var retval = false;
            var optSelected = document.getElementById("hdnOptSelected").value;

            if (fsoApp == undefined) {
                fsoApp = window.document.plugins[0];
            }
            if (fsoApp == undefined) {
                fsoApp = document.applets[0];
            }

            try {
                disallowOp('Content1');
                switch (optSelected) {
                    case "FS":
                        retval = SaveFileSystem();
                        break;
                    case "URL":
                        retval = SaveDocumentImgLink();
                        break;
                    case "URLSD":
                        retval = SaveRecordLink();
                        break;
                    case "CL":
                        retval = ClipBoardDocumentImgLink();
                        break;
                    case "CLSD":
                        retval = ClipBoardRecordLink();
                        break;
                    default:
                        retval = SaveFileSystem();
                        break;
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

        function confirmActionSocket() {
            var retval = false;
            var optSelected = document.getElementById("hdnOptSelected").value;

            try {
                disallowOp('Content1');
                switch (optSelected) {
                    case "FS":
                    case "PKG":
                        SaveFileSystemSocket(function (retVal, connection) {
                            //alert("SaveFileSystem retVal" + retVal);
                            if (retVal) {
                                ShowSuccess();
                                reallowOp();
                            }
                            connection.close();
                        });
                        break;
                    case "URL":
                        retval = SaveDocumentImgLinkSocket(function (retVal, connection) {
                            //alert("SaveDocumentImgLink retVal" + retVal);
                            if (retVal) {
                                ShowSuccess();
                                reallowOp();
                            }
                            connection.close();
                        });
                        break;
                    case "URLSD":
                        retval = SaveRecordLinkSocket(function (retVal, connection) {
                            //alert("SaveRecordLink retVal" + retVal);
                            if (retVal) {
                                ShowSuccess();
                                reallowOp();
                            }
                            connection.close();
                        });
                        break;
                    case "CL":
                        ClipBoardDocumentImgLinkSocket(function (retVal, connection) {
                            if (retVal) {
                                ShowSuccess();
                                reallowOp();
                            }
                            connection.close();
                        });
                        break;
                    case "CLSD":
                        ClipBoardRecordLinkSocket(function (retVal, connection) {
                            if (retVal) {
                                ShowSuccess();
                                reallowOp();
                            }
                            connection.close();
                        });
                        break;
                    default:
                        SaveFileSystemSocket(function (retVal, connection) {
                            if (retVal) {
                                ShowSuccess();
                                reallowOp();
                            }
                            connection.close();
                        });
                        break;
                }


            }
            catch (ex) {
                alert(ex.toString());
                reallowOp();
                retval = false;
            }
        }

        function setSessionValue(key, value) {
        $.ajax({
            type: "POST",
            url: "../handler/SessionHandler.ashx",
            data: { sessionKey: key, sessionValue: value },
            // DO NOT SET CONTENT TYPE to json
            // contentType: "application/json; charset=utf-8", 
            // DataType needs to stay, otherwise the response object
            // will be treated as a single string
            dataType: "json"
        });
    }

        function SaveFileSystem() {
            var retval = false;
            var filePath = FisicalFilePath();

            if (filePath != null && filePath != '') {
                try {
                    var optSelected = document.getElementById("hdnOptSelected").value;
                    var _zip = optSelected == "PKG" ? "1" : "0"
                    setSessionValue("DownloadZipPackageDocument", _zip);
                    //console.log("SETTATA " + _zip);
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
                    retval = false;
                }
            }

            return retval;
        }

        function SaveDocumentImgLink() {
            specialExtension = 'URL';
            var filePath = FisicalFilePath();
            var linkID = "<%= this.Link %>";
        var fileContent = new Array("[InternetShortcut]", "URL=" + linkID, "IconFile=" + "<%= this.FileIcona %>", "IconIndex=1", "[{000214A0-0000-0000-C000-000000000046}]", "HotKey=0", "Prop3=19,2");
            specialExtension = '';

            return fsoApp.createTextFile(filePath, true, fileContent);
        }

        function SaveRecordLink() {
            specialExtension = 'URL';
            var filePath = FisicalFilePath();
            var linkSD = "<%= this.LinkSD %>";
        var fileContent = new Array("[InternetShortcut]", "URL=" + linkSD, "IconFile=" + "<%= this.FileIcona %>", "IconIndex=1", "[{000214A0-0000-0000-C000-000000000046}]", "HotKey=0", "Prop3=19,2");
            specialExtension = '';

            return fsoApp.createTextFile(filePath, true, fileContent);
        }

        function ClipBoardDocumentImgLink() {
            var linkID = "<%= this.Link %>";
            return fsoApp.copyToClipboard(linkID);
        }

        function ClipBoardRecordLink() {
            var linkSD = "<%= this.LinkSD %>";
            return fsoApp.copyToClipboard(linkSD);
        }

        function GetFileName() {
            var fileName = "";
            var ext = '';

            var txtName = document.getElementById("txtFileName").value;

            if (txtName != '') {
                if (specialExtension == '')
                    ext = GetFileExtension();
                else
                    ext = specialExtension;

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
        //NO APPLET INIT
        function SaveFileSystemSocket(callback) {
            var retval = false;
            FisicalFilePathSocket(function (filePath, callback) {
                //console.log('ZIP 1 SaveFileSystemSocket')
                if (filePath != null && filePath != '') {
                    try {
                        var encodedFilePath = EncodeHtml(filePath);


                        var paramPost = "<<filePath:" + encodedFilePath + ">>";
                        var urlPost = '../CheckInOutApplet/SaveFilePage.aspx?issocket=true';
                        var optSelected = document.getElementById("hdnOptSelected").value;
                        //console.log("SELECED: " + optSelected);
                        $.ajax({
                            type: 'POST',
                            url: urlPost,
                            data: { "filePath": encodedFilePath, "package" : optSelected == "PKG" ? "1" : "0" },
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
                    }
                    catch (ex) {
                        //ajaxDialogModal('File_DownloadError', 'error', '');
                        alert('<%=this.GetMessage("DOWNLOAD_ERROR")%>:\n' + ex.toString());
                    retval = false;
                    callback(retval);
                }
            }

        });



        }

        function SaveDocumentImgLinkSocket(callback) {
            specialExtension = 'URL';
            FisicalFilePathSocket(function (filePath) {
                var linkID = "<%= this.Link %>";
            var fileContent = new Array("[InternetShortcut]", "URL=" + linkID, "IconFile=" + "<%= this.FileIcona %>", "IconIndex=1", "[{000214A0-0000-0000-C000-000000000046}]", "HotKey=0", "Prop3=19,2");
            specialExtension = '';
            createTextFile(filePath, true, fileContent, function (retVal, connection) {

                if (retVal === 'true') {
                    ShowSuccess();
                    reallowOp();
                }
                connection.close();
            });

        });
        }

        function SaveRecordLinkSocket(callback) {
            specialExtension = 'URL';
            FisicalFilePathSocket(function (filePath) {
                var linkSD = "<%= this.LinkSD %>";
            var fileContent = new Array("[InternetShortcut]", "URL=" + linkSD, "IconFile=" + "<%= this.FileIcona %>", "IconIndex=1", "[{000214A0-0000-0000-C000-000000000046}]", "HotKey=0", "Prop3=19,2");
            specialExtension = '';
            createTextFile(filePath, true, fileContent, function (retVal, connection) {

                if (retVal === 'true') {
                    ShowSuccess();
                    reallowOp();
                }
                connection.close();
            });
        });
        }

        function ClipBoardDocumentImgLinkSocket(callback) {
            var linkID = "<%= this.Link %>";
            copyToClipBoard(linkID, callback);
        }

        function ClipBoardRecordLinkSocket(callback) {
            var linkSD = "<%= this.LinkSD %>";
            copyToClipBoard(linkSD, callback);
        }

        function GetFileName() {
            var fileName = "";
            var ext = '';

            var txtName = document.getElementById("txtFileName").value;

            if (txtName != '') {
                if (specialExtension == '')
                    ext = GetFileExtension();
                else
                    ext = specialExtension;

                if (ext.toUpperCase() == "P7M")
                    fileName = txtName + "<%=this.GetP7mFileExtensions()%>";
                else
                    fileName = txtName + "." + ext;
            }

            return fileName;
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

        function setTempFolderSocket() {

            disallowOp('Content1');
            getSpecialFolder(function (folderPath, connection) {
                connection.close();
                if (folderPath != null && folderPath != '') {
                    document.getElementById("txtFolderPath").value = shortfolderPath(folderPath);
                }
                reallowOp();
            });
        }

        function SelectFolderSocket() {


            if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1 || isMsie()) {
                SelectFolderSocketFirefox();
            } else {
                var actualFolderValue = document.getElementById("txtFolderPath").value;
                var btnBrowseForFolder = document.getElementById("btnBrowseForFolder");

                btnBrowseForFolder.disabled = true;
                //var folder = fsoApp.selectFolder
                selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolderValue, function (folder, connection) {
                if (folder != "") {
                    folderExists(folder,
                        function (exist, connection) {
                            if ("true" === exist) {
                                document.getElementById("txtFolderPath").value = folder;
                                btnBrowseForFolder.disabled = false;
                            }
                            else {
                                if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {

                                    createFolder(folder, function (create, connection) {
                                        if ("true" === create) {
                                            document.getElementById("txtFolderPath").value = folder;
                                        }
                                        else {
                                            ajaxDialogModal('Path_AccessDenied', 'error', '');
                                            document.getElementById("txtFolderPath").value = '';
                                        }
                                        btnBrowseForFolder.disabled = false;
                                        connection.close();
                                    });
                                } else {
                                    btnBrowseForFolder.disabled = false;
                                }
                            }
                            connection.close();
                        });
                } else {
                    btnBrowseForFolder.disabled = false;
                }
                connection.close();
            });
            }
        }

        function SelectFolderSocketFirefox() {
            var actualFolder = document.getElementById("txtFolderPath").value;
            selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolder, function (folder, connection) {
            if (folder != "") {
                folderExists(folder,
                    function (exist, connection) {
                        if ("true" === exist) {
                            document.getElementById("txtFolderPath").value = folder;
                        }
                        else {
                            if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {

                                createFolder(folder, function (create, connection) {
                                    if ("true" === create) {
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


        function EncodeHtml(value) {
            value = escape(value);
            value = value.replace(/\//g, "%2F");
            value = value.replace(/\?/g, "%3F");
            value = value.replace(/=/g, "%3D");
            value = value.replace(/&/g, "%26");
            value = value.replace(/@/g, "%40");
            return value;
        }

        function ShowSuccess() {
            ajaxDialogModal('ChechInOutSuccess', 'check', '');
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <asp:UpdatePanel ID="pnlAppletTag" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <applet id='fsoApplet'
                code='com.nttdata.fsoApplet.gui.FsoApplet'
                codebase='<%=Page.ResolveClientUrl("~/Libraries/")%>'
                archive='FsoApplet.jar'
                width='10' height='9'>
                <param name="java_arguments" value="-Xms128m" />
                <param name="java_arguments" value="-Xmx512m" />
            </applet>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
            <div>
                <asp:UpdatePanel ID="rblSavingOption" class="row" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="lblSavingOption" runat="server" /><br />
                        <asp:RadioButtonList ID="rblListSavingOption" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblSavingOption_SelectedIndexChanged"
                            RepeatDirection="Vertical">
                            <asp:ListItem ID="optFileSystem" Value="FS" Selected="True" runat="server"></asp:ListItem>
                            <asp:ListItem ID="optPackage" Value="PKG" runat="server"></asp:ListItem>
                            <asp:ListItem ID="optClipboard" Value="CL" runat="server"></asp:ListItem>
                            <asp:ListItem ID="optClipboardSD" Value="CLSD" runat="server"></asp:ListItem>
                            <asp:ListItem ID="optSaveUrl" Value="URL" runat="server"></asp:ListItem>
                            <asp:ListItem ID="optSaveUrlSD" Value="URLSD" runat="server"></asp:ListItem>
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <br />
            <asp:UpdatePanel ID="udpFileSystem" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div>
                        <asp:Label ID="lblFolderPath" runat="server"></asp:Label><br />
                        <asp:TextBox ID="txtFolderPath" runat="server" Width="400px" ClientIDMode="Static"></asp:TextBox>
                        <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." ClientIDMode="Static"></asp:Button>
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
                OnMouseOver="btnHover" Text="Procedi" ClientIDMode="Static" OnClick="CheckInOutConfirmButton_Click" />
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="CheckInOutCloseButton_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
